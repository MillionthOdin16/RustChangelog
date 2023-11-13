using UnityEngine;
using UnityEngine.UI;

public class UIGenesDisplay : MonoBehaviour
{
	public UIGene[] GeneUI;

	public Text[] TextLinks;

	public Text[] TextDiagLinks;

	public void Init(GrowableGenes genes)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		GrowableGene[] genes2 = genes.Genes;
		foreach (GrowableGene gene in genes2)
		{
			GeneUI[num].Init(gene);
			num++;
			if (num < genes.Genes.Length)
			{
				((Graphic)TextLinks[num - 1]).color = (genes.Genes[num].IsPositive() ? GeneUI[num - 1].PositiveColour : GeneUI[num - 1].NegativeColour);
			}
		}
	}

	public void InitDualRow(GrowableGenes genes, bool firstRow)
	{
		if (firstRow)
		{
			InitFirstRow(genes);
		}
		else
		{
			InitSecondRow(genes);
		}
	}

	private void InitFirstRow(GrowableGenes genes)
	{
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		GrowableGene[] genes2 = genes.Genes;
		foreach (GrowableGene growableGene in genes2)
		{
			if (growableGene.Type != growableGene.PreviousType)
			{
				GeneUI[num].InitPrevious(growableGene);
			}
			else
			{
				GeneUI[num].Init(growableGene);
			}
			num++;
			if (num >= genes.Genes.Length)
			{
				break;
			}
			if (growableGene.Type != growableGene.PreviousType || genes.Genes[num].Type != genes.Genes[num].PreviousType)
			{
				((Behaviour)TextLinks[num - 1]).enabled = false;
				continue;
			}
			((Behaviour)TextLinks[num - 1]).enabled = true;
			((Graphic)TextLinks[num - 1]).color = (genes.Genes[num].IsPositive() ? GeneUI[num - 1].PositiveColour : GeneUI[num - 1].NegativeColour);
		}
	}

	private void InitSecondRow(GrowableGenes genes)
	{
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		GrowableGene[] genes2 = genes.Genes;
		foreach (GrowableGene growableGene in genes2)
		{
			if (growableGene.Type != growableGene.PreviousType)
			{
				GeneUI[num].Init(growableGene);
			}
			else
			{
				GeneUI[num].Hide();
			}
			num++;
			if (num >= genes.Genes.Length)
			{
				break;
			}
			((Behaviour)TextLinks[num - 1]).enabled = false;
			GrowableGene growableGene2 = genes.Genes[num];
			((Behaviour)TextDiagLinks[num - 1]).enabled = false;
			if (growableGene.Type != growableGene.PreviousType && growableGene2.Type != growableGene2.PreviousType)
			{
				((Behaviour)TextLinks[num - 1]).enabled = true;
				((Graphic)TextLinks[num - 1]).color = (growableGene2.IsPositive() ? GeneUI[num - 1].PositiveColour : GeneUI[num - 1].NegativeColour);
			}
			else if (growableGene.Type == growableGene.PreviousType && growableGene2.Type != growableGene2.PreviousType)
			{
				ShowDiagLink(num - 1, -43f, growableGene2);
			}
			else if (growableGene.Type != growableGene.PreviousType && growableGene2.Type == growableGene2.PreviousType)
			{
				ShowDiagLink(num - 1, 43f, growableGene2);
			}
		}
	}

	private void ShowDiagLink(int index, float rotation, GrowableGene nextGene)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localEulerAngles = ((Component)TextDiagLinks[index]).transform.localEulerAngles;
		localEulerAngles.z = rotation;
		((Component)TextDiagLinks[index]).transform.localEulerAngles = localEulerAngles;
		((Behaviour)TextDiagLinks[index]).enabled = true;
		((Graphic)TextDiagLinks[index]).color = (nextGene.IsPositive() ? GeneUI[index].PositiveColour : GeneUI[index].NegativeColour);
	}
}
